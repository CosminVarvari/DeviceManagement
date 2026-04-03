import { Component, OnInit, OnDestroy  } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';
import { Router } from '@angular/router';
import { BehaviorSubject, Subject, combineLatest } from 'rxjs';
import { takeUntil, map } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog } from '@angular/material/dialog';
import { DeviceService } from '../../../core/services/device.service';
import { Device } from '../../../core/models/device.model';
import { ConfirmDialogComponent } from '../../../shared/components/confirm-dialog/confirm-dialog.component';

@Component({
  selector: 'app-devices-page',
  templateUrl: './devices-page.component.html',
  styleUrls: ['./devices-page.component.scss']
})
export class DevicesPageComponent implements OnInit {

  currentUserName = this.authService.getCurrentUser()?.name || 'User';

  private destroy$     = new Subject<void>();
  private devices$     = new BehaviorSubject<Device[]>([]);
  private searchQuery$ = new BehaviorSubject<string>('');
  private typeFilter$  = new BehaviorSubject<'All' | 'Phone' | 'Tablet'>('All');

  filteredDevices: Device[] = [];
  loading = true;
  displayedColumns = ['name', 'manufacturer', 'type', 'os', 'ram', 'assignedTo', 'assign', 'actions'];

  constructor(
    private deviceService: DeviceService,
    public authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {}

  ngOnInit(): void {
    combineLatest([this.devices$, this.searchQuery$, this.typeFilter$])
      .pipe(
        map(([devices, query, type]) => {
          let result = devices;

          if (type !== 'All') {
            result = result.filter(d => d.type === type);
          }

          if (query.trim()) {
            const q = query.toLowerCase().trim();
            result = result.filter(d =>
              d.name.toLowerCase().includes(q)              ||
              d.manufacturer.toLowerCase().includes(q)     ||
              (d.assignedUserName?.toLowerCase().includes(q) ?? false)
            );
          }

          return result;
        }),
        takeUntil(this.destroy$)
      )
      .subscribe(filtered => this.filteredDevices = filtered);

    this.loadDevices();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadDevices(): void {
    this.loading = true;
    this.deviceService.getAll()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  devices => { 
          this.devices$.next(devices); 
          this.loading = false; 
        },
        error: ()      => { 
          this.showError('Failed to load devices.'); 
          this.loading = false; 
        }
      });
  }

  onSearchChange(query: string): void     { 
    this.searchQuery$.next(query); 
  }

  onTypeFilterChange(type: any): void     { 
    this.typeFilter$.next(type); 
  }

  viewDetail(id: string): void            { 
    this.router.navigate(['/devices', id]); 
  }
  
  openCreateForm(): void                  { 
    this.router.navigate(['/devices/new']); 
  }

  openEditForm(device: Device, event: Event): void {
    event.stopPropagation();
    this.router.navigate(['/devices', device.id, 'edit']);
  }

  confirmDelete(device: Device, event: Event): void {
    event.stopPropagation();
    const dialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '380px',
      data: {
        title:       'Delete Device',
        message:     `Are you sure you want to delete "${device.name}"?`,
        confirmText: 'Delete'
      }
    });

    dialogRef.afterClosed()
      .pipe(takeUntil(this.destroy$))
      .subscribe(confirmed => { if (confirmed) this.deleteDevice(device.id); });
  }

  private deleteDevice(id: string): void {
    this.deviceService.delete(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  () => {
          this.devices$.next(this.devices$.value.filter(d => d.id !== id));
          this.showSuccess('Device deleted successfully.');
        },
        error: () => this.showError('Failed to delete device.')
      });
  }

  assign(device: Device, event: Event): void {
    event.stopPropagation();
    this.deviceService.assign(device.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  updated => {
          this.devices$.next(this.devices$.value.map(d => d.id === updated.id ? updated : d));
          this.showSuccess(`"${device.name}" assigned to you.`);
        },
        error: err => this.showError(err.error?.message ?? 'Failed to assign.')
      });
  }

  unassign(device: Device, event: Event): void {
    event.stopPropagation();
    this.deviceService.unassign(device.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next:  updated => {
          this.devices$.next(this.devices$.value.map(d => d.id === updated.id ? updated : d));
          this.showSuccess(`"${device.name}" unassigned.`);
        },
        error: err => this.showError(err.error?.message ?? 'Failed to unassign.')
      });
  }

  isAssignedToCurrentUser(device: Device): boolean {
    return device.assignedUserId === this.authService.getCurrentUser()?.id;
  }

  private showSuccess(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 3000, panelClass: 'snack-success' });
  }

  private showError(msg: string): void {
    this.snackBar.open(msg, 'Close', { duration: 4000, panelClass: 'snack-error' });
  }
}
