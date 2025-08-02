// src/app/pages/login/login.component.ts

import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

// Imports nécessaires pour un composant standalone avec des formulaires
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, // Pour *ngIf, etc.
    FormsModule   // Pour ngModel, ngForm, etc.
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  // Objet pour stocker les données du formulaire
  credentials = {
    matricule: '',
    password: ''
  };

  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    if (!this.credentials.matricule || !this.credentials.password) {
      this.errorMessage = "Veuillez remplir tous les champs.";
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    this.authService.login(this.credentials.matricule, this.credentials.password).subscribe({
      next: (response) => {
        // Connexion réussie
        this.isLoading = false;
        console.log('Connexion réussie, token:', response.token);
        // Rediriger vers le tableau de bord ou une autre page protégée
        this.router.navigate(['/']); 
      },
      error: (err) => {
        // Gestion des erreurs
        this.isLoading = false;
        if (err.status === 401 || err.status === 400) {
          this.errorMessage = "Matricule ou mot de passe invalide.";
        } else {
          this.errorMessage = "Une erreur est survenue. Veuillez réessayer plus tard.";
        }
        console.error('Erreur de connexion:', err);
      }
    });
  }
}
