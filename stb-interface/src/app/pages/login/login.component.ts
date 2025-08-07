import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../services/auth.service';

// Imports nécessaires pour un composant standalone avec des formulaires
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginDto } from '../../models/LoginDto';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule, // Pour *ngIf, etc.
    FormsModule,
    RouterModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {

  // Objet pour stocker les données du formulaire, maintenant de type LoginDto
  // Nous allons utiliser 'identifier' comme champ temporaire pour la saisie
  // et le mapper à 'matricule' ou 'email' dans onSubmit.
  credentials: LoginDto = {
    matricule: '',
    password: ''
  };

  // Champ pour la saisie de l'utilisateur (peut être matricule ou email)
  identifierInput: string = '';

  isLoading = false;
  errorMessage: string | null = null;

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    // Réinitialiser les champs matricule et email de credentials
    this.credentials.matricule = '';
    this.credentials.email = '';

    // Déterminer si l'entrée est un email ou un matricule
    if (this.identifierInput.includes('@')) {
      this.credentials.email = this.identifierInput;
    } else {
      this.credentials.matricule = this.identifierInput;
    }

    // La validation doit être adaptée pour vérifier soit matricule, soit email
    if ((!this.credentials.matricule && !this.credentials.email) || !this.credentials.password) {
      this.errorMessage = "Veuillez remplir l'identifiant (matricule ou email) et le mot de passe.";
      return;
    }

    this.isLoading = true;
    this.errorMessage = null;

    // Appel de la nouvelle méthode login du service qui accepte un LoginDto
    this.authService.login(this.credentials).subscribe({
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
          this.errorMessage = "Identifiant ou mot de passe invalide.";
        } else {
          this.errorMessage = "Une erreur est survenue. Veuillez réessayer plus tard.";
        }
        console.error('Erreur de connexion:', err);
      }
    });
  }
ngOnInit() {
  if (typeof document !== 'undefined') {
    document.body.classList.add('no-header');
  }
}

ngOnDestroy() {
  if (typeof document !== 'undefined') {
    document.body.classList.remove('no-header');
  }
}


}