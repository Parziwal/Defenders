import { Component, OnInit } from '@angular/core';
import {AddOrEditCommentDto, CaffClient, CaffDetailsDto, CommentClient} from "../../api/api.generated";
import {ActivatedRoute} from "@angular/router";
import {AuthService} from "../../auth/auth.service";
import {ToastrService} from "ngx-toastr";

@Component({
  selector: 'app-details-page',
  templateUrl: './details-page.component.html',
  styleUrls: ['./details-page.component.css']
})
export class DetailsPageComponent implements OnInit {
  private readonly _caffId: number;
  public caff?: CaffDetailsDto;
  public commentText?: string;
  public isAdmin = false;

  constructor(private readonly _caffService: CaffClient,
              private _route: ActivatedRoute,
              private readonly _commentService: CommentClient,
              private toastr: ToastrService,
              public authService: AuthService) {
    this._caffId =  +this._route.snapshot.params['id'];
  }

  ngOnInit(): void {
    this.getCaffDetails();
    this.isAdmin = this.authService.isAdmin;
  }

  private getCaffDetails() {
    this._caffService.getCaffDetails(this._caffId).subscribe(response => {
      this.caff = response;
    });
  }

  public addNewComment() {
   this._commentService.addCommentToCaff(this._caffId, new AddOrEditCommentDto({commentText: this.commentText})).subscribe(
     () => this.showSuccess("Komment hozzáadva!"),
     () => this.showError("Komment hozzáadása sikertelen")
   );
   this.getCaffDetails();
  }

  public deleteComment(commentId: number) {
    this._commentService.deleteComment(commentId).subscribe(() => {
        this.caff!.comments = this.caff?.comments?.filter(x => x.id !== commentId);
        this.showSuccess("Törlés sikeres!");
    }, () => {
        this.showError("Törlés sikertelen")
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
      this.showSuccess("Letöltés sikeres!");
    }, () => {
      this.showError("Letöltés sikertelen")
    })
  }

  showSuccess(text: string) {
    this.toastr.success(text, 'Sikeres művelet!');
  }

  showError(text: string) {
    this.toastr.error(text, 'Művelet sikertelen!');
  }

}
