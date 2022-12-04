import { Component, OnInit } from '@angular/core';
import {AddOrEditCommentDto, CaffClient, CaffDetailsDto, CommentClient} from "../../api/api.generated";
import {ActivatedRoute, Router} from "@angular/router";
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
              public authService: AuthService,
              private _router: Router,
              ) {
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
     () => { this.showSuccess("Komment hozzáadva!"); this.getCaffDetails(); },
     () => this.showError("Komment hozzáadása sikertelen")
   );
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

  public updateComment(commentId: number, text: string) {
    this._commentService.editComment(commentId,new AddOrEditCommentDto({commentText: text})).subscribe(
      () => {
        this.caff!.comments = this.caff?.comments?.map(x => {
          if (x.id === commentId) {
            x.text = text;
            return x;
          }
          return x;
        }); 
        this.showSuccess("Komment szerkesztése sikeres!");
      },
      () => this.showError("Komment szerkesztése sikertelen")
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

  public deleteCaff() {
    this._caffService.deleteCaff(this._caffId).subscribe((response) => {
      this.showSuccess("Törlés sikeres!");
      this._router.navigate(['list']);
    }, () => {
      this.showError("Törlés sikertelen")
    })
  }

  showSuccess(text: string) {
    this.toastr.success(text, 'Sikeres művelet!');
  }

  showError(text: string) {
    this.toastr.error(text, 'Művelet sikertelen!');
  }

}
