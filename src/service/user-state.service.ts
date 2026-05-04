import { Injectable } from '@angular/core';

export interface UserState {
  name: string;
  email: string;
  phoneNumber: number;
  state: string;
  country: string;
  isExistingCustomer?: boolean;
  customerId?: string;
}

@Injectable({
  providedIn: 'root'
})
export class UserStateService {
  private userState: UserState | null = null;

  setUserState(state: UserState) {
    this.userState = state;
  }

  getUserState(): UserState | null {
    return this.userState;
  }

  clearUserState() {
    this.userState = null;
  }

  isUserInitialized(): boolean {
    return this.userState !== null;
  }
}
