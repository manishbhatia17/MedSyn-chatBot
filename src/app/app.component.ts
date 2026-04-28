import * as signalR from '@microsoft/signalr';
import { Component, ElementRef, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ChatService, UserInfoModel, chatMessage } from '../service/ChatService/chat.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../environments/environment';
import { Observable, Observer } from 'rxjs';
import { ChatMessage, ChatRule } from 'src/model/chatMessage';
import {  ActionType, OptionModel, RuleMeta } from 'src/model/optionModel';
import { ChatEngineService } from 'src/service/ChatService/chatEngine.service';
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
  isExistingCustomer = false; // UI-only flag
  optionsShownTime: Date | null = null;

  chatMessages: ChatMessage[] = [];


  chatForm = this.fb.group({
    name: ['', [Validators.required]],
    phoneNumber: ['', [Validators.required, Validators.pattern('^\\d{10,}$')]],
    email: ['', [Validators.required, Validators.pattern('^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$')]],
    state: ['', [Validators.required]],
    country: ['', [Validators.required]],
    isCurrentCustomer: [false],
    customerId: [''],
  });
  messageToSend: string = '';
  isLoading = false;
  private hubConnection: signalR.HubConnection;

  userId = null;
  departmentId = '';
  errorSummary = ""
  constructor(private fb: FormBuilder,
    private chatService: ChatService,
    private chatEngine: ChatEngineService
  ) {
  }
  ngOnInit() {
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

  OnResponseFromAdmin = (message: string) => {
    this.chatMessages.push({ isIncoming: true, message: message });
    this.scrollToBottom();
  }


  private normalizeCustomerId() {
    const isCurrentCustomer = !!this.chatForm.get('isCurrentCustomer')?.value;
    const customerIdControl = this.chatForm.get('customerId');
    if (!isCurrentCustomer) {
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
    this.normalizeCustomerId();
    // Log the raw form value for debugging/UI-only flow
    console.log('Form value on Next:', this.chatForm.value);
    if (this.chatForm.valid) {
      let chatData: UserInfoModel = {
        name: this.chatForm.get('name').value,
        email: this.chatForm.get('email').value,
        phoneNumber: parseInt(this.chatForm.get('phoneNumber').value),
        state: this.chatForm.get('state')?.value,
        country: this.chatForm.get('country')?.value,
        isCurrentCustomer: !!this.chatForm.get('isCurrentCustomer')?.value,
        customerId: this.chatForm.get('isCurrentCustomer')?.value
          ? (this.chatForm.get('customerId')?.value ?? '').toString().trim()
          : undefined,
      }


      // UI-only behavior: determine if existing customer based on customerId presence
      this.isExistingCustomer = !!(chatData.isCurrentCustomer && chatData.customerId && chatData.customerId !== '');

      //call the api to stre the data in backend and then push the greeting and options to user
      //------------------------------------------ do it later 

      this.handleUserResponse(RuleMeta.MainMenuOption.intent,null);
      
        this.IsUserDataSubmited = true;
        this.optionsShownTime = new Date();
        // ensure options visible and scroll to bottom so options and later selected messages are visible
        setTimeout(() => this.scrollToBottom(), 80);
      
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
  this.chatMessages.push({ isIncoming, message, options, time: new Date() });
  setTimeout(() => this.scrollToBottom(), 50);
}

AddMessageToChat(message: string, isIncoming: boolean = true) {
  this.chatMessages.push({ isIncoming, message, time: new Date() });
  setTimeout(() => this.scrollToBottom(), 50);
}
  



SendMessage(): void {
    if (this.messageToSend?.trim()) {
     
      //call handle user message to get the response from bot based on the message and current state of conversation
      //const botResponse = this.handleUserMessage(this.messageToSend, { currentRuleId: '', context: {} }, null);
       this.chatMessages.push({ isIncoming: false, message: this.messageToSend });
      this.scrollToBottom();
      this.messageToSend = '';
      console.log(this.chatMessages)

    }
    else {
      this.TimerErrorSummary('Please enter message')
    }
  }




  onOptionSelected(option: OptionModel) {
   
    //find the rule based on the option selected and then call the handle user response to get the response from bot based on the message and current state of conversation
    this.handleUserResponse(option.rule, null);
    setTimeout(() => this.scrollToBottom(), 50);
  }
 

  //Animation methods below
  //----------------------------------------------------
  //----------------------------------------------------
  scrollToBottom(): void {
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

  ngOnDestroy(): void {

  }
}



// Easing function for smooth animation
function easeInOutCubic(t: number): number {
  return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
}