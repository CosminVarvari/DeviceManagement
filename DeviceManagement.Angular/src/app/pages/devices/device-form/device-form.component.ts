import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { takeUntil } from 'rxjs/internal/operators/takeUntil';
import { Subject } from 'rxjs/internal/Subject';
import { DeviceService } from 'src/app/core/services/device.service';

@Component({
  selector: 'app-device-form',
  templateUrl: './device-form.component.html',
  styleUrls: ['./device-form.component.scss']
})
export class DeviceFormComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  form!: FormGroup;
  loading  = false;
  isEdit   = false;
  deviceId: string | null = null;

  get pageTitle():   string { return this.isEdit ? 'Edit Device'    : 'Add New Device'; }
  get submitLabel(): string { return this.isEdit ? 'Save Changes'   : 'Create Device';  }

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    private snackBar: MatSnackBar
  ) {}

  ngOnInit(): void {
    this.buildForm();

    const id = this.route.snapshot.paramMap.get('id');
    if (id && id !== 'new') {
      this.isEdit   = true;
      this.deviceId = id;
      this.loadDevice(id);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private buildForm(): void {
    this.form = this.fb.group({
      name:            ['', [Validators.required, Validators.maxLength(150)]],
      manufacturer:    ['', [Validators.required, Validators.maxLength(100)]],
      type:            ['', Validators.required],
      operatingSystem: ['', Validators.required],
      osVersion:       ['', Validators.required],
      processor:       ['', Validators.required],
      ramAmount:       [null, [Validators.required, Validators.min(1), Validators.max(512)]],
      description:     ['']
    });
  }

  private loadDevice(id: string): void {
    this.loading = true;
    this.deviceService.getById(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  device => { this.form.patchValue(device); this.loading = false; },
        error: ()     => {
          this.snackBar.open('Device not found.', 'Close', { duration: 3000 });
          this.router.navigate(['/devices']);
        }
      });
  }

  onSubmit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.loading = true;
    const request$ = this.isEdit
      ? this.deviceService.update(this.deviceId!, this.form.value)
      : this.deviceService.create(this.form.value);

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: device => {
        this.snackBar.open(
          this.isEdit ? 'Device updated.' : 'Device created.',
          'Close',
          { duration: 3000, panelClass: 'snack-success' }
        );
        this.router.navigate(['/devices']);
      },
      error: err => {
        this.snackBar.open(
          err.error?.message ?? 'Something went wrong.',
          'Close',
          { duration: 4000, panelClass: 'snack-error' }
        );
        this.loading = false;
      }
    });
  }

  goBack(): void {
    this.router.navigate(['/devices']);
  }

  hasError(field: string, error: string): boolean {
    const control = this.form.get(field);
    return !!(control?.hasError(error) && control.touched);
  }
}
