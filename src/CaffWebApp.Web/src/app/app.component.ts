import { Component, OnInit } from '@angular/core';
import {AuthService} from "./auth/auth.service";

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  user: any;

  constructor(public authService: AuthService) {
   

  }
  ngOnInit() {
    this.user = this.authService.currentUser()?.profile.name;
  }

  title = 'CaffWebApp.Web';
}

