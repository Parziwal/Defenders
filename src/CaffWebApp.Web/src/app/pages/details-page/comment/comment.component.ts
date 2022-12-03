import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {AddOrEditCommentDto, CommentClient, CommentDto} from "../../../api/api.generated";

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit {
  @Input() public comment?: CommentDto;
  @Output() commentDeleted = new EventEmitter<number>();
  editing = false
  public commentText?: string;
  constructor(private readonly _commentService: CommentClient) { }

  ngOnInit(): void {
  }

  public editComment() {
    this.editing = !this.editing;
  }

  public updateComment(commentId: number) {
    this.editing = false
    this._commentService.editComment(commentId,new AddOrEditCommentDto({commentText: this.commentText})).subscribe();
  }


  public deleteComment(commentId: number) {
    this.commentDeleted.emit(commentId);
  }

}
