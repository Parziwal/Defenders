import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { CaffClient } from './api/api.generated';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  public forecasts?: WeatherForecast[];

  constructor(private readonly _service: CaffClient) {
   // this._service.listCaffImages().subscribe();
  }

  title = 'CaffWebApp.Web';
}

interface WeatherForecast {
  date: string;
  temperatureC: number;
  temperatureF: number;
  summary: string;
}
