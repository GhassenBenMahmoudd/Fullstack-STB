import { CommonModule } from '@angular/common'; // ✅ ajoute ceci
import { ActivatedRoute, Router, RouterOutlet } from '@angular/router';
import { FooterComponent } from './footer/footer.component';
import { AboutComponent } from './about/about.component';
import { ConditionComponent } from './condition/condition.component';
import { ParticiperComponent } from './participer/participer.component';
import { HeaderComponent } from './header/header.component';
import { Component } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule, // ✅ indispensable pour *ngIf
    RouterOutlet,
    HeaderComponent,
    FooterComponent
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'stb-interface';
constructor(public router: Router,private route: ActivatedRoute,private authService: AuthService)  {
  this.route.queryParams.subscribe(params => {
    const token = params['token'];
    if (token) {
      localStorage.setItem('authToken', token);
      this.authService.setUserFromToken(token); // Méthode pour décoder le JWT et stocker user
      this.router.navigate(['/']); // Redirige vers home sans query
    }
  });
}}
