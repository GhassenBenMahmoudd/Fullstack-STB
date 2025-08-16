import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
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

  constructor(private router: Router) {}

  ngOnInit(): void {
  if (typeof window !== 'undefined') {
    const user = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (user && user.token) {
      this.isLoggedIn = true;
      this.userName = user.name || 'Nom';
      this.userPreNom = user.prenom || '';
      this.userRole = user.role || 'Utilisateur';
    }
  }
}


  logout(): void {
    // Supprimer les données utilisateur stockées
    localStorage.removeItem('currentUser');
    this.isLoggedIn = false;
    this.userName = '';
    this.userPreNom = '';
    this.userRole = '';

    // Redirection vers la page de login ou accueil
    this.router.navigate(['/login']);
  }
}
