import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ActivatedRoute, Router } from '@angular/router';
import { catchError, of, Subject, switchMap, takeUntil, tap } from 'rxjs';
import { Device } from 'src/app/core/models/device.model';
import { AuthService } from 'src/app/core/services/auth.service';
import { DeviceService } from 'src/app/core/services/device.service';
import { ConfirmDialogComponent } from 'src/app/shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-device-detail',
  templateUrl: './device-detail.component.html',
  styleUrls: ['./device-detail.component.scss']
})
export class DeviceDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  device: Device | null = null;
  loading = true;
  aiOverview = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private deviceService: DeviceService,
    public authService: AuthService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    this.getDeviceDetails();
  }

  getDeviceDetails() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.deviceService.getById(id).pipe(
      tap(device => this.device = device),
      switchMap(device => {
        const descriptionRequest = {
          name:            device.name,
          manufacturer:    device.manufacturer,
          type:            device.type,
          operatingSystem: device.operatingSystem,
          osVersion:       device.osVersion,
          processor:       device.processor,
          ramAmount:       device.ramAmount
        };
        return this.deviceService.generateAIOverview(descriptionRequest).pipe(
          catchError(() => {
            this.aiOverview = '';
            return of({ description: '' });
          })
        );
      }),
      takeUntil(this.destroy$)
    ).subscribe({
      next:  response => {
        this.aiOverview = response.description;
        this.loading    = false;
      },
      error: () => {
        this.showError('Device not found.');
        this.router.navigate(['/devices']);
      }
    });
  }

  goBack(): void { 
    this.router.navigate(['/devices']); 
  }

  confirmDelete(): void {
    this.dialog.open(ConfirmDialogComponent, {
      width: '380px',
      data: {
        title: 'Delete Device',
        message: `Are you sure you want to delete "${this.device!.name}"?`,
        confirmText: 'Delete'
      }
    })
    .afterClosed()
    .pipe(takeUntil(this.destroy$))
    .subscribe(confirmed => {
      if (!confirmed) return;
      this.deviceService.delete(this.device!.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next:  () => { this.showSuccess('Device deleted.'); this.router.navigate(['/devices']); },
          error: () => this.showError('Failed to delete device.')
        });
    });
  }

  assign(): void {
    this.deviceService.assign(this.device!.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  updated => { this.device = updated; this.showSuccess('Device assigned to you.'); },
        error: err     => this.showError(err.error?.message ?? 'Failed to assign.')
      });
  }

  unassign(): void {
    this.deviceService.unassign(this.device!.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  updated => { this.device = updated; this.showSuccess('Device unassigned.'); },
        error: err     => this.showError(err.error?.message ?? 'Failed to unassign.')
      });
  }

  isAssignedToCurrentUser(): boolean {
    return this.device?.assignedUserId === this.authService.getCurrentUser()?.id;
  }

  private showSuccess(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 3000, panelClass: 'snack-success' });
  }

  private showError(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 4000, panelClass: 'snack-error' });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
