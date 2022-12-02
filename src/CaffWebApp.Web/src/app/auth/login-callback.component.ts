import { Component, OnInit } from '@angular/core';
import { AuthService } from './auth.service';
import {Router} from "@angular/router";

@Component({
  selector: 'app-login-callback',
  templateUrl: './login-callback.component.html',
})
export class LoginCallbackComponent implements OnInit {

  constructor(private router:Router, private authService: AuthService) { }

  ngOnInit() {
    this.authService.completeAuthentication();
    this.router.navigate(['/list']);
  }

}
