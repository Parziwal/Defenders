import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LogoutCallbackComponent } from './auth/logout-callback.component';
import { HomeComponent } from './pages/home/home.component';
import { LoginCallbackComponent } from './auth/login-callback.component';
import {AdminPageComponent} from "./pages/admin-page/admin-page.component";
import {DetailsPageComponent} from "./pages/details-page/details-page.component";
import {ListPageComponent} from "./pages/list-page/list-page.component";
import {AuthGuardService} from "./auth/auth-guard.service";


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
    canActivate: [AuthGuardService, AdminGuardService]
  },
  {
    path: 'details/:id',
    component: DetailsPageComponent,
    canActivate: [AuthGuardService]
  },
  {
    path: 'list',
    component: ListPageComponent,
    canActivate: [AuthGuardService]
  },
];
@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
