import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { CreateDeviceRequest, Device, GenerateDescriptionRequest, UpdateDeviceRequest } from '../models/device.model';

@Injectable({
  providedIn: 'root'
})
export class DeviceService {
  private readonly baseUrl = `${environment.apiUrl}/devices`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Device[]> {
    return this.http.get<Device[]>(this.baseUrl);
  }

  getById(id: string): Observable<Device> {
    return this.http.get<Device>(`${this.baseUrl}/${id}`);
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }

  assign(deviceId: string): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/${deviceId}/assign`, null);
  }

  unassign(deviceId: string): Observable<Device> {
    return this.http.post<Device>(`${this.baseUrl}/${deviceId}/unassign`, null);
  }

  create(dto: CreateDeviceRequest): Observable<Device> {
    return this.http.post<Device>(this.baseUrl, dto);
  }

  update(id: string, dto: UpdateDeviceRequest): Observable<Device> {
    return this.http.put<Device>(`${this.baseUrl}/${id}`, dto);
  }

  generateAIOverview(dto: GenerateDescriptionRequest): Observable<{ description: string }> {
    return this.http.post<{ description: string }>(`${this.baseUrl}/generate-description`, dto);
  }
}