import { Component, Input, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { async } from 'rxjs';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member;
  isOnline: boolean = false;
  users: string[];

  constructor(private memberService: MembersService, private toastr: ToastrService, public presence: PresenceService) {

      }

  ngOnInit(): void {

    this.presence.onlineUsers$.pipe(take(1)).subscribe(users => {
      this.users = users;
      if (this.users!)
      this.isOnline = this.users.indexOf(this.member.username) > -1;
     
       
   })

    //this.users?.forEach(item => console.log(item));
    //console.log(this.member.username);
  }

   
 
  

  addLike(member: Member){
    console.log(member.username);
    this.memberService.addLike(member.username).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    })
  }

}
