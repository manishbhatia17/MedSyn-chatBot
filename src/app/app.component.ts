import * as signalR from '@microsoft/signalr';
import { Component, ElementRef, Input, OnDestroy, OnInit, ViewChild, ViewEncapsulation } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ChatService, ChatUserModel, chatMessage, chatMessageFromClient } from '../service/ChatService/chat.service';
import { ToastrService } from 'ngx-toastr';
import { environment } from '../environments/environment';
import { Observable, Observer } from 'rxjs';
import { ChatMessage } from 'src/model/chatMessage';
import { OptionActions, OptionModel } from 'src/model/optionModel';

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

  chatRoomId = '';
  userId = null;
  departmentId = '';
  errorSummary = ""
  constructor(private fb: FormBuilder,
    private chatService: ChatService) {
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
      let chatData: ChatUserModel = {
        name: this.chatForm.get('name').value,
        email: this.chatForm.get('email').value,
        phoneNumber: parseInt(this.chatForm.get('phoneNumber').value),
        companyId: parseInt(this.companyId),
        state: this.chatForm.get('state')?.value,
        country: this.chatForm.get('country')?.value,
        isCurrentCustomer: !!this.chatForm.get('isCurrentCustomer')?.value,
        customerId: this.chatForm.get('isCurrentCustomer')?.value
          ? (this.chatForm.get('customerId')?.value ?? '').toString().trim()
          : undefined,
      }

      // Log the constructed payload
      console.log('Chat data payload:', chatData);

      // UI-only behavior: determine if existing customer based on customerId presence
      this.isExistingCustomer = !!(chatData.isCurrentCustomer && chatData.customerId && chatData.customerId !== '');

      // Push initial bot greeting into messages and scroll to it
       this.chatMessages.push({ isIncoming: true, message: "Hi, we're here to help you.", time: new Date() });
    this.chatMessages.push({
      isIncoming: true,
      message: "Select one of the options below or type your query",
      options: this.chatService.mainMenu,
      time: new Date()
    });
    //   // The select prompt is now rendered by the options block above the options list
      // so we don't push it into chatMessages here.

      // After a short delay show options bubble (so greeting appears first)
      setTimeout(() => {
        this.IsUserDataSubmited = true;
        this.optionsShownTime = new Date();
        // ensure options visible and scroll to bottom so options and later selected messages are visible
        setTimeout(() => this.scrollToBottom(), 80);
      }, 300);

    } else {
      this.chatForm.markAllAsTouched();
      this.TimerErrorSummary("Please enter valid data")

      this.isLoading = false;
    }
  }

  TimerErrorSummary(message) {
    this.errorSummary = message;
    setTimeout(() => {
      this.errorSummary = '';
    }, 3000)

  }

  SendMessage(): void {
    if (this.messageToSend != null && this.messageToSend != undefined && this.messageToSend.trim() != '') {
      let chatmessage: chatMessageFromClient = {
        chatRoomId: this.chatRoomId,
        message: this.messageToSend,
        userId: this.userId,
        companyId: this.companyId,
        departmentId: this.departmentId

      }


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
    switch (option.action) {
      case OptionActions.REQUEST_PRODUCT_INFO:
        this.chatMessages.push({
          isIncoming: true,
          message: `I will help you with that. 

          Please enter the product name or type of product you're interested in.
          `,
          time: new Date() });
        break;
      case OptionActions.VIEW_ORDER_STATUS:
        this.chatMessages.push({ isIncoming: true, message: "I want to view my order status", time: new Date() });
        break;
      case OptionActions.PLACE_ORDER:
        this.chatMessages.push({ isIncoming: true  , message: "I want to place an order", time: new Date() });
        break;
      case OptionActions.REQUEST_REP_INFO:
        this.chatMessages.push({ isIncoming: true, message: "I want to request contact information for my rep", time: new Date() });
        break;
      case OptionActions.LEAVE_MESSAGE:
        this.chatMessages.push({ isIncoming: true, message: `Thats great
           
          Please write your message in text and we will appricate your message `, time: new Date() });
        break;
      default:
        this.chatMessages.push({ isIncoming: true, message: `Please write your question in the text box below`, time: new Date() });
    }
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
  ngOnDestroy(): void {

  }
}



// Easing function for smooth animation
function easeInOutCubic(t: number): number {
  return t < 0.5 ? 4 * t * t * t : (t - 1) * (2 * t - 2) * (2 * t - 2) + 1;
}