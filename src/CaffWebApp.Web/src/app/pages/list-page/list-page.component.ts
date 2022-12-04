import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { CaffClient, CaffDto } from '../../api/api.generated';

@Component({
  selector: 'app-list-page',
  templateUrl: './list-page.component.html',
  styleUrls: ['./list-page.component.css'],
})
export class ListPageComponent implements OnInit {
  list: CaffDto[] = [];
  constructor(private readonly service: CaffClient, private router: Router, private toastr: ToastrService) {
    service.listCaffImages().subscribe((result) => (this.list = result));
  }

  ngOnInit(): void {}

  goToDetails(element: any) {
    this.router.navigate(['/details', element.id]);
  }

  searchTerm = ""
  searchCaffImages(){
    this.service.listCaffImages(this.searchTerm).subscribe(result => this.list=result)
  }

  onFileSelected(event:any) {
    const file: File = event.target.files[0];
    if (file) {
      this.service.uploadCaffFile({data: file,fileName: file.name}).subscribe(
        () => { this.showSuccess("Sikeres feltöltés!"); this.searchCaffImages(); },
        () => this.showError("Sikertelen feltöltés!")
      );
    }
  }

  showSuccess(text: string) {
    this.toastr.success(text, 'Sikeres művelet!');
  }

  showError(text: string) {
    this.toastr.error(text, 'Művelet sikertelen!');
  }
}
