import { Component, OnInit } from '@angular/core';
import {CaffClient} from "../../api/api.generated";

@Component({
  selector: 'app-list-page',
  templateUrl: './list-page.component.html',
  styleUrls: ['./list-page.component.css'],
})
export class ListPageComponent implements OnInit {
  constructor(private readonly _service: CaffClient) {

  }

  list = [
    {
      "id": 1,
      "fileName": "Cica.caff",
      "creatorName": "Random Ronaldo",
      "createdAt": "2022-11-30T20:20:23.227Z",
      "uploadedBy": "Random Ronaldo",
      "uploadedAt": "2022-11-30T20:20:23.227Z",
      "captions": [
        "Nagyon aranyos", "Legjobb barát"
      ],
      "tags": [
        "állat", "hobby"
      ]
    },
    {
      "id": 2,
      "fileName": "Kutya.caff",
      "creatorName": "Random Ronaldo",
      "createdAt": "2022-11-30T20:20:23.227Z",
      "uploadedBy": "Random Ronaldo",
      "uploadedAt": "2022-11-30T20:20:23.227Z",
      "captions": [
        "Nagyon aranyos", "Legjobb barát"
      ],
      "tags": [
        "állat", "hobby"
      ]
    }
  ];


  ngOnInit(): void {}
}
