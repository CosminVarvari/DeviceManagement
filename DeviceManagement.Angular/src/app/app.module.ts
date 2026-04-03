import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { MaterialModule } from './material.module';
import { LoginComponent } from './pages/auth/login/login.component';
import { DevicesPageComponent } from './pages/devices/devices-page/devices-page.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HTTP_INTERCEPTORS, HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RegisterComponent } from './pages/auth/register/register.component';
import { ConfirmDialogComponent } from './shared/components/confirm-dialog/confirm-dialog.component';
import { AuthInterceptor } from './core/interceptors/auth.interceptor';
import { DeviceDetailComponent } from './pages/devices/device-detail/device-detail.component';
import { DeviceFormComponent } from './pages/devices/device-form/device-form.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    DevicesPageComponent,
    RegisterComponent,
    ConfirmDialogComponent,
    DeviceDetailComponent,
    DeviceFormComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    MaterialModule,
    ReactiveFormsModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
