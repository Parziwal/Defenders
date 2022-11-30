import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DeatailsPageComponent } from './deatails-page.component';

describe('DeatailsPageComponent', () => {
  let component: DeatailsPageComponent;
  let fixture: ComponentFixture<DeatailsPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DeatailsPageComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(DeatailsPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
