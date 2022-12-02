import { Component } from '@angular/core';
import {AuthService} from "./auth/auth.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  user: any;


  constructor(private _service: AuthService) {
    this._service.currentUser().then(resp => {
      this.user = resp?.profile.name;
      //this.user = resp?.profile['role'];
    });

  }

  title = 'CaffWebApp.Web';
}

