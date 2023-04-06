import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import {catchError, of, tap} from "rxjs";

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html'
})
export class FetchDataComponent {
  public forecasts: WeatherForecast[] = [];

  constructor(http: HttpClient, @Inject('API_URL') apiUrl: string) {
    http.get<WeatherForecast[]>(apiUrl + 'weatherforecast')
      .pipe(
        tap(result => this.forecasts = result),
        catchError(error => of(console.error(error)))
      ).subscribe();
  }
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
