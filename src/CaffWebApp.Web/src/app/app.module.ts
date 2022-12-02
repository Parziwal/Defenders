import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';


import { AppComponent } from './app.component';
import { HomeComponent } from './pages/home/home.component';
import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from './auth/auth.module';
import { AdminPageComponent } from './pages/admin-page/admin-page.component';
import { DetailsPageComponent } from './pages/details-page/details-page.component';
import { ListPageComponent } from './pages/list-page/list-page.component';

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AdminPageComponent,
    DetailsPageComponent,
    ListPageComponent
    
  ],
  imports: [
    BrowserModule, HttpClientModule, AppRoutingModule, AuthModule,FormsModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
