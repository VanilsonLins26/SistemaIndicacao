import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule,RouterModule, ReactiveFormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  registerForm! : FormGroup;

  ngOnInit(): void{
    this.registerForm = this.fb.group({
      nome: ['', [Validators.required]],
      email: ['', [Validators.required, Validators.email]],
      senha: ['', [Validators.required, Validators.pattern('^(?=.*[A-Za-z])(?=.*\\d).{8,}$')]],
      codigoIndicacao: ['']

    });

    this.route.queryParams.subscribe(params => {
      const refCode = params['ref'];
      if (refCode) {
        this.registerForm.get('codigoIndicacao')?.setValue(refCode);
      }
    });
  }

  onSubmit(): void {
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: () => {
          this.router.navigate(['/perfil']);
        },
        error: (err) => {
          console.error('Erro no cadastro', err);
          
          alert(`Falha no cadastro: ${err.error?.message || 'Tente novamente.'}`);
        }
      });
    } else {
      this.registerForm.markAllAsTouched();
    }
    
  }

}
