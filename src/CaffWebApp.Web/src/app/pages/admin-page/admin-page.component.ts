import { Component, OnInit } from '@angular/core';
import { UserClient } from "../../api/api.generated";

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent implements OnInit {

  constructor(private readonly _service: UserClient) { }

  users = [
    {
      "id": "1",
      "fullName": "Kis Károly",
      "email": "kis.karcsi@gmail.com"
    },
    {
      "id": "2",
      "fullName": "Nagy Károly",
      "email": "nagy.karcsi@gmail.com"
    }
  ]

  ngOnInit(): void {
  }

}
