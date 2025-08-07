import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { RegisterDto } from '../models/RegisterDto';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  registerData: RegisterDto = {
    Prenom: '',
    Nom: '',
    Email: '',
    NumeroTelephone: '',
    Password: ''
  };

  isLoading = false;
  errorMessage: string | null = null;
  successMessage: string | null = null;

  constructor(private authService: AuthService, private router: Router) {}

  onSubmit() {
    this.isLoading = true;
    this.errorMessage = null;

    this.authService.register(this.registerData).subscribe({
      next: () => {
        this.successMessage = "Inscription réussie. Veuillez vérifier votre email.";
        this.isLoading = false;
        setTimeout(() => this.router.navigate(['/login']), 2000);
      },
      error: (err) => {
        this.isLoading = false;
        this.errorMessage = "Erreur d'inscription. Veuillez vérifier vos données.";
        console.error(err);
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
