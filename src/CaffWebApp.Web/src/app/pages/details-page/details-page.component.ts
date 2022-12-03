import { Component, OnInit } from '@angular/core';
import {AddOrEditCommentDto, CaffClient, CaffDetailsDto, CommentClient} from "../../api/api.generated";
import {ActivatedRoute} from "@angular/router";

@Component({
  selector: 'app-details-page',
  templateUrl: './details-page.component.html',
  styleUrls: ['./details-page.component.css']
})
export class DetailsPageComponent implements OnInit {
  private readonly _caffId: number;
  public caff?: CaffDetailsDto;
  public commentText?: string;

  constructor(private readonly _caffService: CaffClient,
              private _route: ActivatedRoute,
              private readonly _commentService: CommentClient) {
    this._caffId =  +this._route.snapshot.params['id'];
  }

  ngOnInit(): void {
    this.getCaffDetails();
  }

  private getCaffDetails() {
    this._caffService.getCaffDetails(this._caffId).subscribe(response => {
      this.caff = response;
    });
  }

  public addNewComment() {
   this._commentService.addCommentToCaff(this._caffId, new AddOrEditCommentDto({commentText: this.commentText})).subscribe();
   this.getCaffDetails();
  }

  public deleteComment(commentId: number) {
    this._commentService.deleteComment(commentId).subscribe(() => {
        this.caff!.comments = this.caff?.comments?.filter(x => x.id !== commentId);
      }
    );
  }

  public downloadFile() {
    this._caffService.downloadCaffFile(this._caffId).subscribe((response) => {
      let filename: string = `${this._caffId}.caff`;
      let binaryData = [];
      binaryData.push(response.data);
      let downloadLink = document.createElement('a');
      downloadLink.href = window.URL.createObjectURL(new Blob(binaryData, { type: 'blob' }));
      downloadLink.setAttribute('download', filename);
      document.body.appendChild(downloadLink);
      downloadLink.click();
    })
  }

}
