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