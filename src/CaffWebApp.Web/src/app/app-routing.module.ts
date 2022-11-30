import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { LogoutCallbackComponent } from './auth/logout-callback.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginCallbackComponent } from './auth/login-callback.component';
import {AdminPageComponent} from "./pages/admin-page/admin-page.component";
import {DetailsPageComponent} from "./pages/details-page/details-page.component";
import {ListPageComponent} from "./pages/list-page/list-page.component";


const routes: Routes = [
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: 'home',
    component: HomeComponent,
  },
  {
    path: 'login-callback',
    component: LoginCallbackComponent,
  },
  {
    path: 'logout-callback',
    component: LogoutCallbackComponent,
  },
  {
    path: 'admin',
    component: AdminPageComponent,
  },
  {
    path: 'details',
    component: DetailsPageComponent,
  },
  {
    path: 'list',
    component: ListPageComponent,
  },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
