import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { AuthService } from '../services/auth.service';  // adapte le chemin

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterModule],
  templateUrl: './header.component.html',
})
export class HeaderComponent implements OnInit {
  isLoggedIn = false;
  userName = '';
  userPreNom = '';
  userRole = '';

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
      this.userName = user?.nom || '';
      this.userPreNom = user?.prenom || '';
      this.userRole = user?.role || '';
    });
  }

  logout(): void {
    this.authService.logout();
    // Redirection après logout, par exemple :
    window.location.href = '/';
    // ou si tu as le Router, utilise router.navigateByUrl('/')
  }
  
}
