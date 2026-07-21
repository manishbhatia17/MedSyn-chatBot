import * as signalR from '@microsoft/signalr';
import { Component, ElementRef, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ChatService, UserInfoModel, chatMessage, CustomerChatRequest, CustomerChatResponse } from '../service/ChatService/chat.service';
import { UserStateService } from '../service/user-state.service';
import { LocationService } from '../service/location.service';
import { Country, State } from 'country-state-city';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../environments/environment';
import { Observable, Observer } from 'rxjs';
import { ChatMessage, ChatRule } from 'src/model/chatMessage';
import {  ActionType, OptionModel, RuleMeta } from 'src/model/optionModel';
import { ChatEngineService } from 'src/service/ChatService/chatengine.service';
import { ChatState } from 'src/model/chatState';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
  encapsulation: ViewEncapsulation.ShadowDom,
  standalone: false
})
export class AppComponent implements OnInit, OnDestroy {

  @ViewChild('scrollContainerMessage', { static: false }) scrollContainerMessage: ElementRef;
  @Input() companyId: string;
  IsChatBot = false;
  IsUserDataSubmited = false;
  optionsShownTime: Date | null = null;
  isExistingCustomer: boolean = false;
  chatMessages: ChatMessage[] = [];

  countries: any[] = [];
  states: any[] = [];
  selectedCountry: string = '';


  chatForm = this.fb.group({
    name: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern('^\\d{10,}$')]],
    email: ['', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]],
    state: ['', [Validators.required]],
    country: ['', [Validators.required]],
    isExistingCustomer: [false],
    customerId: [''],
  });
  messageToSend: string = '';
  isLoading = false;
  chatLogId: number = null;
  currentFunctionHint: string = null;
  private hubConnection: signalR.HubConnection;

  userId = null;
  departmentId = '';
  errorSummary = '';
  private readonly STORAGE_KEY = 'medgyn_chat_state';

  constructor(
    private fb: FormBuilder,
    private chatService: ChatService,
    private chatEngine: ChatEngineService,
    private locationService: LocationService,
    private userStateService: UserStateService
  ) {}
  ngOnInit() {
    this.countries = this.locationService.getCountries();
    this.selectedCountry = 'US';
    this.chatForm.get('country')?.setValue('US');
    this.states = this.locationService.getStates('US');
    this.loadChatState();
    // ...existing code...
    //comment when used for prod 
    // this.companyId = '1004';
    // this.IsChatBot = true;
    // this.IsUserDataSubmited = true;
    // this.chatMessages.push({ isIncoming: true, message: "Hi, we're here to help you.", time: new Date() });
    // this.chatMessages.push({
    //   isIncoming: true,
    //   message: "Select one of the options below or type your query",
    //   options: this.chatService.mainMenu,
    //   time: new Date()
    // });
    //this.chatMessages.push({ isIncoming: false, message: "I have following query", time: new Date() });
  }

  onCountryChange(event: any) {
    this.selectedCountry = event.target.value;
    this.states = this.locationService.getStates(this.selectedCountry);
    this.chatForm.get('state')?.setValue('');
  }

  OnResponseFromAdmin = (message: string) => {
    this.chatMessages.push({ isIncoming: true, message: message });
    this.scrollToBottom();
  }


  private normalizeCustomerId() {
    const isExistingCustomer = !!this.chatForm.get('isExistingCustomer')?.value;
    const customerIdControl = this.chatForm.get('customerId');
    if (!isExistingCustomer) {
      customerIdControl?.setValue('');
      customerIdControl?.setErrors(null);
      return;
    }

    const customerId = (customerIdControl?.value ?? '').toString().trim();
    if (!customerId) {
      customerIdControl?.setErrors({ required: true });
    } else {
      customerIdControl?.setErrors(null);
    }
  }

  ChatBotToggle() {
    this.IsChatBot = this.IsChatBot ? false : true
  }

  submitchatForm() {
    this.errorSummary = '';
    this.normalizeCustomerId();
    // Log the raw form value for debugging/UI-only flow
    console.log('Form value on Next:', this.chatForm.value);
    if (this.chatForm.valid) {
      let chatData: UserInfoModel = {
        name: this.chatForm.get('name').value,
        email: this.chatForm.get('email').value,
        phoneNumber: this.chatForm.get('phoneNumber').value,
        state: this.chatForm.get('state')?.value,
        country: this.chatForm.get('country')?.value,
        isExistingCustomer: !!this.chatForm.get('isExistingCustomer')?.value,
        customerId: this.chatForm.get('isExistingCustomer')?.value
          ? parseInt((this.chatForm.get('customerId')?.value ?? '').toString().trim(), 10)
          : undefined,
        companyId: this.companyId,
      }

      this.userStateService.setUserState(chatData);
      this.isExistingCustomer = chatData.isExistingCustomer;
      this.isLoading = true;

      this.chatService.LogChatCustomer(chatData).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.chatLogId = response.chatLogId;
          this.isExistingCustomer = response.isExistingCustomer;
          this.handleUserResponse(RuleMeta.MainMenuOption.intent, null);
          this.IsUserDataSubmited = true;
          this.optionsShownTime = new Date();
          this.saveChatState();
          setTimeout(() => this.scrollToBottom(), 80);
        },
        error: (err) => {
          this.isLoading = false;
          this.errorSummary = err?.error && typeof err.error === 'string'
            ? err.error
            : 'Unable to connect. Please try again.';
        }
      });
    } else {
      this.chatForm.markAllAsTouched();
      this.TimerErrorSummary("Please enter valid data")

      this.isLoading = false;
    }
  }

  
  handleUserResponse(ruleIntent: string, state: ChatState ): any {

  //check if the if have the ruleIntenet and find the chatbotrule
   const intent: ChatRule = this.chatEngine.detectIntent(ruleIntent);
  
  if(intent){
    //handle the response based on the action type of the rule
    switch(intent.action){
      case ActionType.DisplayMenu:
        this.AddMessageMenuToChat(intent.response, true, intent.actionPayload as OptionModel[]);
        break;  
      case ActionType.DisplayMessage:
        this.AddMessageToChat(intent.response, true);
        break;
     default:
      this.chatMessages.push({
        isIncoming: true,
        message: `Sorry, I am not able to process your request at the moment.`,
        time: new Date()
      });
    }
  }

}



AddMessageMenuToChat(message: string, isIncoming: boolean = true, options?: OptionModel[]) {
    if (options && options.length > 0 && !this.isExistingCustomer) {
      options = options.filter(option => option.optionalData?.isOptionForNewCustomer === true);
    }
  this.chatMessages.push({ isIncoming, message, options, time: new Date() });
  this.saveChatState();
  setTimeout(() => this.scrollToBottom(), 50);
}

AddMessageToChat(message: string, isIncoming: boolean = true) {
  this.chatMessages.push({ isIncoming, message, time: new Date() });
  this.saveChatState();
  setTimeout(() => this.scrollToBottom(), 50);
}
  



SendMessage(): void {
    if (this.messageToSend?.trim()) {
      const messageText = this.messageToSend;
      this.AddMessageToChat(messageText, false);
      this.messageToSend = '';
      this.isLoading = true;

      const hint = this.currentFunctionHint;
      this.currentFunctionHint = null;

      this.chatService.SendChatMessage({ chatLogId: this.chatLogId, message: messageText, functionHint: hint, companyId: this.companyId }).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.AddMessageToChat(response.message, true);
          // If the backend returned no data, the operation wasn't completed —
          // restore the hint so the next message retries the same function
          if (!response.data && hint) {
            this.currentFunctionHint = hint;
          }
        },
        error: () => {
          this.isLoading = false;
          this.AddMessageToChat('Sorry, I was unable to process your request. Please try again.', true);
        }
      });
    } else {
      this.TimerErrorSummary('Please enter message');
    }
  }




  onOptionSelected(option: OptionModel) {
    if (option.action === ActionType.ExternalLink) {
      window.open(option.value, '_blank');
      return;
    }

    const rule = this.chatEngine.detectIntent(option.rule);

    if (rule?.action === ActionType.CallAPI) {
      this.AddMessageToChat(option.label, false);
      this.isLoading = true;
      this.chatService.SendChatMessage({
        chatLogId: this.chatLogId,
        message: option.label,
        functionHint: rule.actionPayload as string,
        companyId: this.companyId
      }).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.AddMessageToChat(response.message, true);
        },
        error: () => {
          this.isLoading = false;
          this.AddMessageToChat('Sorry, I was unable to process your request. Please try again.', true);
        }
      });
    } else {
      this.currentFunctionHint = option.rule ?? null;
      this.handleUserResponse(option.rule, null);
    }

    setTimeout(() => this.scrollToBottom(), 50);
  }
 

  onMessageAreaClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    const action = target.getAttribute('data-chataction');
    if (!action) return;

    const rule = this.chatEngine.detectIntent(RuleMeta.REQUEST_REP_INFO.id);
    if (rule?.action === ActionType.CallAPI) {
      this.AddMessageToChat('Speak to Sales Rep', false);
      this.isLoading = true;
      this.chatService.SendChatMessage({
        chatLogId: this.chatLogId,
        message: 'Speak to Sales Rep',
        functionHint: rule.actionPayload as string,
        companyId: this.companyId
      }).subscribe({
        next: (response) => {
          this.isLoading = false;
          this.AddMessageToChat(response.message, true);
        },
        error: () => {
          this.isLoading = false;
          this.AddMessageToChat('Sorry, I was unable to process your request. Please try again.', true);
        }
      });
    }
  }

  //Animation methods below
  //----------------------------------------------------
  //----------------------------------------------------
  scrollToBottom(): void {
    if (!this.scrollContainerMessage) return;
    const element = this.scrollContainerMessage.nativeElement;
    const duration = 500; // Animation duration in milliseconds
    this.animateScroll(element, element.scrollHeight, duration).subscribe();
  }
  animateScroll(element: HTMLElement, to: number, duration: number): Observable<number> {
    const start = element.scrollTop;
    const change = to - start;
    const startTime = performance.now();

    return new Observable((observer: Observer<number>) => {
      const animateScroll = (timestamp: number) => {
        const elapsedTime = timestamp - startTime;
        const progress = Math.min(elapsedTime / duration, 1);
        const easedProgress: any = easeInOutCubic(progress);

        element.scrollTop = start + change * easedProgress;

        if (progress < 1) {
          requestAnimationFrame(animateScroll);
        } else {
          observer.complete();
        }
      };

      requestAnimationFrame(animateScroll);
    });
  }
  TimerErrorSummary(message) {
    this.errorSummary = message;
    setTimeout(() => {
      this.errorSummary = '';
    }, 3000)

  }

  private saveChatState(): void {
    const state = {
      chatMessages: this.chatMessages,
      chatLogId: this.chatLogId,
      IsUserDataSubmited: this.IsUserDataSubmited,
      isExistingCustomer: this.isExistingCustomer,
    };
    localStorage.setItem(this.STORAGE_KEY, JSON.stringify(state));
  }

  private loadChatState(): void {
    try {
      const raw = localStorage.getItem(this.STORAGE_KEY);
      if (!raw) return;
      const state = JSON.parse(raw);
      this.chatMessages = (state.chatMessages ?? []).map((m: any) => ({
        ...m,
        time: m.time ? new Date(m.time) : undefined,
      }));
      this.chatLogId = state.chatLogId ?? null;
      this.IsUserDataSubmited = state.IsUserDataSubmited ?? false;
      this.isExistingCustomer = state.isExistingCustomer ?? false;
      if (this.IsUserDataSubmited) {
        setTimeout(() => this.scrollToBottom(), 80);
      }
    } catch {
      localStorage.removeItem(this.STORAGE_KEY);
    }
  }

  startNewChat(): void {
    localStorage.removeItem(this.STORAGE_KEY);
    this.chatMessages = [];
    this.chatLogId = null;
    this.IsUserDataSubmited = false;
    this.isExistingCustomer = false;
    this.currentFunctionHint = null;
    this.errorSummary = '';
    this.chatForm.reset({
      name: '', email: '', phoneNumber: '',
      state: '', country: 'US',
      isExistingCustomer: false, customerId: '',
    });
    this.selectedCountry = 'US';
    this.states = this.locationService.getStates('US');
  }

  ngOnDestroy(): void {

  }
}



// Easing function for smooth animation
function easeInOutCubic(t: number): number {
  return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
}