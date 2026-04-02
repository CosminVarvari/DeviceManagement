import { Component, OnInit } from '@angular/core';
import { AuthService } from 'src/app/core/services/auth.service';

@Component({
  selector: 'app-devices-page',
  templateUrl: './devices-page.component.html',
  styleUrls: ['./devices-page.component.scss']
})
export class DevicesPageComponent implements OnInit {

  currentUserName = this.authService.getCurrentUser()?.name || 'User';

  constructor(public authService: AuthService) { }

  ngOnInit(): void {}
}
