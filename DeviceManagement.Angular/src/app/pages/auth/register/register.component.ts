import { Component } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.scss']
})
export class RegisterComponent {
  form: FormGroup;
  loading = false;
  hide = true;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar
  ) {
    this.form = this.fb.group({
      name:     ['', [Validators.required, Validators.maxLength(100)]],
      email:    ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required,
        Validators.minLength(8),
        Validators.pattern(/(?=.*[A-Z])(?=.*[0-9])/)
      ]],
      location: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    this.authService.register(this.form.value).subscribe({
      next: () => {
        this.snackBar.open('Account created! Please sign in.', 'Close', { duration: 3000 });
        this.router.navigate(['/login']);
      },
      error: err => {
        this.snackBar.open(
          err.error?.message ?? 'Registration failed.',
          'Close',
          { duration: 4000 }
        );
        this.loading = false;
      }
    });
  }
}