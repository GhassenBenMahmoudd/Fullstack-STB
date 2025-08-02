import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptorsFromDi } from '@angular/common/http'; // <-- Importez ceci
import { HTTP_INTERCEPTORS } from '@angular/common/http'; // <-- Et ceci

import { AppComponent } from './app/app.component';
import { routes } from './app/app.routes'; // Assurez-vous que le nom du fichier est correct
import { AuthInterceptor } from './app/interceptors/auth.interceptor'; // <-- Importez votre intercepteur

bootstrapApplication(AppComponent, {
  providers: [
    // 1. Fournir les routes de l'application
    provideRouter(routes ),

    // 2. Fournir le client HTTP et activer les intercepteurs basés sur les classes
    provideHttpClient(withInterceptorsFromDi()), 

    // 3. Enregistrer votre intercepteur (comme dans AppModule)
    { 
      provide: HTTP_INTERCEPTORS, 
      useClass: AuthInterceptor, 
      multi: true 
    }
  ]
}).catch(err => console.error(err));
