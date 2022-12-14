import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';
import {CommentDto} from "../../../api/api.generated";

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})
export class CommentComponent implements OnInit {
  @Input() public comment?: CommentDto;
  @Input() public isAdmin: boolean = false;
  @Output() commentDeleted = new EventEmitter<number>();
  @Output() commentUpdated = new EventEmitter<{id: number, text: string}>();
  editing = false
  public commentText?: string;
  constructor() { }

  ngOnInit(): void {
  }

  public editComment() {
    this.editing = !this.editing;
  }

  public updateComment(commentId: number) {
    this.editing = false
    this.commentUpdated.emit({id: commentId, text: this.commentText!});
  }


  public deleteComment(commentId: number) {
    this.commentDeleted.emit(commentId);
  }

}
