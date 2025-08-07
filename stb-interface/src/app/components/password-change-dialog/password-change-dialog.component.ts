import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-password-change-dialog',
  template: `
    <h2>Changer le mot de passe</h2>
    <form>
      <label for="password">Nouveau mot de passe :</label>
      <input id="password" type="password" [(ngModel)]="password" name="password" required>
      <br>
      <button type="button" (click)="onClose()">Fermer</button>
      <button type="submit" (click)="onChangePassword()">Valider</button>
    </form>
  `,
  standalone: true,
  imports: [FormsModule],
})
export class PasswordChangeDialogComponent {
  password = '';

  constructor(private dialogRef: MatDialogRef<PasswordChangeDialogComponent>) {}

  onClose() {
    this.dialogRef.close();
  }

  onChangePassword() {
    // TODO: Ajouter la logique de changement de mot de passe
    this.dialogRef.close(this.password);
  }
}
