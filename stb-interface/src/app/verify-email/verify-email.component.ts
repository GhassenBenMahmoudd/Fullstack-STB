import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../services/auth.service';
import { MatDialog } from '@angular/material/dialog';

import { PasswordChangeDialogComponent } from '../components/password-change-dialog/password-change-dialog.component';

@Component({
  selector: 'app-verify-email',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './verify-email.component.html',
  styleUrls: ['./verify-email.component.css']
})
export class VerifyEmailComponent implements OnInit {

  private route = inject(ActivatedRoute);
  private authService = inject(AuthService);
  private router = inject(Router);
  private dialog = inject(MatDialog);

  errorMessage = '';

  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const token = params['token'];
      if (token) {
        this.authService.verifyEmail(token).subscribe({
          next: (userData) => {
            this.authService.setUser(userData); // connecter le user (stockage localStorage + BehaviorSubject)
            this.dialog.open(PasswordChangeDialogComponent); // ouvrir popup de mot de passe
            this.router.navigate(['/']); // ou tableau de bord
          },
          error: () => {
            this.errorMessage = 'Lien invalide ou expiré.';
          }
        });
      }
    });
  }
}
