import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/login/login.component';
import { DevicesPageComponent } from './pages/devices/devices-page/devices-page.component';
import { RegisterComponent } from './pages/auth/register/register.component';
import { AuthGuard } from './core/guards/auth.guard';
import { DeviceDetailComponent } from './pages/devices/device-detail/device-detail.component';

const routes: Routes = [
  {
    path: '', redirectTo: 'devices', pathMatch: 'full'
  },
  {
    path: 'login', component: LoginComponent
  },
  {
    path: 'register', component: RegisterComponent
  },
  {
    path: 'devices', canActivate: [AuthGuard], component: DevicesPageComponent
  },
  {
    path: 'devices/:id', canActivate: [AuthGuard], component: DeviceDetailComponent
  },
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
