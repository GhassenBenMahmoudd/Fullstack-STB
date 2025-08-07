import { CommonModule } from '@angular/common'; // ✅ ajoute ceci
import { Router, RouterOutlet } from '@angular/router';
import { FooterComponent } from './footer/footer.component';
import { AboutComponent } from './about/about.component';
import { ConditionComponent } from './condition/condition.component';
import { ParticiperComponent } from './participer/participer.component';
import { HeaderComponent } from './header/header.component';
import { Component } from '@angular/core';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, // ✅ indispensable pour *ngIf
    RouterOutlet,
    HeaderComponent,
    FooterComponent,
    AboutComponent,
    ConditionComponent,
    ParticiperComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'stb-interface';
  constructor(public router: Router) {}
}
