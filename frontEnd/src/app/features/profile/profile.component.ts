import { CommonModule } from '@angular/common';
import { Component, OnInit, inject } from '@angular/core'; 
import { AuthService } from '../../core/services/auth.service';
import { ClipboardService } from '../../core/services/clipboard.service';

@Component({
  selector: 'app-profile',
  standalone: true, 
  imports: [CommonModule],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent implements OnInit { 
  authService = inject(AuthService);
  clipboardService = inject(ClipboardService);
  
  showToast = false;
  isLoading = true; 
  user = this.authService.currentUser;

  ngOnInit(): void {
    this.authService.getProfile().subscribe(() => {
      this.isLoading = false;
    });
  }

  async copyLink(): Promise<void> {
    const code = this.user()?.codigoIndicacao;
    if (!code) return;

    const referralLink = `http://localhost:4200/register?ref=${code}`;
    
    try {
      await this.clipboardService.copyToClipBoard(referralLink);
      this.showToast = true;
      setTimeout(() => {
        this.showToast = false;
      }, 3000);
    } catch (err) {
      alert('Falha ao copiar o link.');
      console.error(err);
    }
  }

  logout(): void {
    this.authService.logout();
  }
}