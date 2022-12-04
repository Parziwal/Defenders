import { APP_INITIALIZER, NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService, initializeAuthService } from './auth.service';

import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthTokenInterceptor } from './auth-token.interceptor';
import { LoginCallbackComponent } from './login-callback.component';
import { LogoutCallbackComponent } from './logout-callback.component';

@NgModule({
  declarations: [
    LoginCallbackComponent,
    LogoutCallbackComponent
  ],
  imports: [
    CommonModule,
  ],
  exports: [
    LoginCallbackComponent,
    LogoutCallbackComponent
  ],
  providers: [
    {
      provide: APP_INITIALIZER,
      useFactory: initializeAuthService,
      deps: [AuthService],
      multi: true,
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthTokenInterceptor,
      multi: true
    }
  ],
})
export class AuthModule { }
