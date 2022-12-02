import { Component, OnInit } from '@angular/core';
import {UserClient, UserDto} from "../../api/api.generated";

@Component({
  selector: 'app-admin-page',
  templateUrl: './admin-page.component.html',
  styleUrls: ['./admin-page.component.css']
})
export class AdminPageComponent implements OnInit {

  constructor(private readonly _service: UserClient) { }
  public users: UserDto[] = [];

  ngOnInit(): void {
    this._service.listAllUsers().subscribe((resp) => {
      this.users = resp;
    });
  }

  public deleteUser(userId?: string) {
    this._service.deleteUser(userId!).subscribe(() => {
        this.users = this.users.filter(x => x.id !== userId);
    });
  }

}
