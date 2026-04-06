export interface Device {
  id: string;
  name: string;
  manufacturer: string;
  type: 'Phone' | 'Tablet';
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: number;
  description: string;
  assignedUserId: string | null;
  assignedUserName: string | null;
}

export interface CreateDeviceRequest {
  name: string;
  manufacturer: string;
  type: 'Phone' | 'Tablet';
  operatingSystem: string;
  osVersion: string;
  processor: string;
  ramAmount: number;
  description: string;
}

export type UpdateDeviceRequest = CreateDeviceRequest;

export interface GenerateDescriptionRequest {
  name:            string;
  manufacturer:    string;
  type:            string;
  operatingSystem: string;
  osVersion:       string;
  processor:       string;
  ramAmount:       number;
}