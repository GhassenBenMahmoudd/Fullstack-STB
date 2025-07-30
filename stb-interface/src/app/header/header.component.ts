import { Component } from '@angular/core';
import { RouterModule } from '@angular/router'; // <-- Assurez-vous que c'est importé

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [RouterModule], // <-- Et présent ici
  templateUrl: './header.component.html',
  // ...
})
export class HeaderComponent { }
