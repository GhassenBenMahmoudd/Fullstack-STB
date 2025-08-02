import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {

  constructor(private authService: AuthService, private router: Router) {}

  canActivate(): boolean {
    if (this.authService.isLoggedIn()) {
      return true; // L'utilisateur est connecté, il peut passer
    } else {
      // L'utilisateur n'est pas connecté, on le redirige vers la page de login
      this.router.navigate(['/login']);
      return false;
    }
  }
}
