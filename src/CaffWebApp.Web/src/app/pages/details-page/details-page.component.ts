import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-details-page',
  templateUrl: './details-page.component.html',
  styleUrls: ['./details-page.component.css']
})
export class DetailsPageComponent implements OnInit {

  constructor() { }

  caff =  {
    "id": 0,
    "creatorName": "string",
    "animationDuration": 0,
    "fileName": "string",
    "createdAt": "2022-11-30T20:27:04.040Z",
    "uploadedBy": {
      "id": "string",
      "fullName": "string",
      "email": "string"
    },
    "uploadedAt": "2022-11-30T20:27:04.040Z",
    "ciffImages": [
      {
        "caption": "string",
        "width": 0,
        "height": 0,
        "tags": [
          "string"
        ]
      }
    ],
    "comments": [
      {
        "id": 0,
        "text": "string",
        "createdBy": "string",
        "createAt": "2022-11-30T20:27:04.040Z"
      }
    ]
  }

  ngOnInit(): void {
  }

}
