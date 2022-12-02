import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CaffClient, CaffDto } from '../../api/api.generated';

@Component({
  selector: 'app-list-page',
  templateUrl: './list-page.component.html',
  styleUrls: ['./list-page.component.css'],
})
export class ListPageComponent implements OnInit {
  list: CaffDto[] = [];
  constructor(private readonly service: CaffClient, private router: Router) {
    service.listCaffImages().subscribe((result) => (this.list = result));
  }

  ngOnInit(): void {}

  goToDetails(element: any) {
    this.router.navigate(['/details', { id: element.id }]);
  }

  searchTerm = ""
  searchCaffImages(){
    this.service.listCaffImages(this.searchTerm).subscribe(result => this.list=result)
  }
  

  onFileSelected(event:any) {
    const file: File = event.target.files[0];
    if (file) {
      this.service.uploadCaffFile({data: file,fileName: 'file.caff'}).subscribe();
    }
  }
}
