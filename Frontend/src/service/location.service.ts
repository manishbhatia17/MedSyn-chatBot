import { Country, State } from 'country-state-city';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class LocationService {
  getCountries() {
    return Country.getAllCountries();
  }
  getStates(countryCode: string) {
    return State.getStatesOfCountry(countryCode);
  }
}
