import { Injectable, OnInit } from '@angular/core';
import { UserManager, User } from 'oidc-client-ts';
import { environment } from 'src/environments/environment';


@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private manager: UserManager;
  private user: User | null = null;

  constructor() {
    this.manager = new UserManager(environment.authSettings)
  }

  public async refreshUser(): Promise<void> {
    this.user = await this.manager.getUser();
  }

  get isLoggedIn(): boolean {
    return this.user != null && !this.user.expired;
  }

  get isAdmin(): boolean {
    return this.user?.profile['role'] == 'Admin';
  }

  get getClaims(): any {
    if (this.user)
      return this.user.profile;

    return null;
  }

  get getAuthorizationHeaderValue(): string {
    if (this.user)
      return this.user.token_type + ' ' + this.user.access_token;

    return '';
  }

  currentUser(): User | null {
    return this.user;
  }

  public async login(): Promise<void> {
    this.manager.signinRedirect();
  }

  public logout(): Promise<void> {
    return this.manager.signoutRedirect();
  }

  startAuthentication(): Promise<void> {
    return this.manager.signinRedirect();
  }

  async completeAuthentication(): Promise<void> {
    this.user = await this.manager.signinRedirectCallback();
  }
}

export function initializeAuthService(authService: AuthService) {
  return async () => {
    return await authService.refreshUser();
  };
}
