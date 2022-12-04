import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router";

@Component({
  selector: 'app-logout-callback',
  template: '<p>home works!</p>',

})
export class LogoutCallbackComponent implements OnInit {

  constructor(private router:Router) { }

  ngOnInit() {

  }

}
