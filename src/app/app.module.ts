import { Injector, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { MatInputModule } from '@angular/material/input';
import { AppComponent } from './app.component';
import { createCustomElement } from '@angular/elements';
import { ToastrModule } from 'ngx-toastr';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { ChatService } from '../service/ChatService/chat.service';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations'; // Import BrowserAnimationsModule
import { APIInterceptor } from '../../apiInterceptor/api.interceptor';
import { CustomerOptionsComponent } from './shared/customer-options/customer-options.component';
import { ChatEngineService } from 'src/service/ChatService/chatEngine.service';

@NgModule({
    declarations: [
        AppComponent
    ],
    bootstrap: [AppComponent],
    imports: [
        BrowserModule,
        FormsModule,
        BrowserAnimationsModule,
        ReactiveFormsModule,
        ToastrModule.forRoot(),
        MatInputModule,
        MatSelectModule,
        MatCardModule,
        CustomerOptionsComponent
    ], providers: [ChatService, 
      ChatEngineService,
      {
            provide: HTTP_INTERCEPTORS,
            useClass: APIInterceptor,
            multi: true
        }, provideHttpClient(withInterceptorsFromDi())] })
export class AppModule {

  constructor(private injector: Injector) {

  }
  ngDoBootstrap() {
    const element = createCustomElement(AppComponent, { injector: this.injector })
    customElements.define('helpdesk-chatbot', element);
  }
}
