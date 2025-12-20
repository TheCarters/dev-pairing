export interface DevGroup {
  id: number;
  name: string;
  identifier: string; // Guid
  createdAt: string;
}

export interface User {
  id: number;
  firstName: string;
  email: string;
  createdAt: string;
}

export interface UserPreferences {
  notifyOnSlotJoin: boolean;
  notifyBeforeSessionStart: boolean;
}

export interface PairingSlot {
  id: number;
  devGroupId: number;
  owner: User;
  startTime: string;
  endTime: string;
  title: string;
  description?: string;
  ntfyTopic?: string;
  signups?: PairingSignup[];
}

export interface PairingSignup {
  id: number;
  slotId?: number;
  slot?: PairingSlot;
  user: User;
  signedUpAt: string;
  status: string;
}

export interface CreateDevGroup {
  name: string;
}

export interface CreateUser {
  firstName: string;
  email: string;
  joinGroupId?: string;
}

export interface JoinGroup {
  userId: number;
  groupIdentifier: string;
}

export interface CreateSlot {
  devGroupId: number;
  ownerId: number;
  startTime: string;
  endTime: string;
  title: string;
  description?: string;
}

export interface UpdateSlot {
  startTime: string;
  endTime: string;
  title: string;
  description?: string;
}

export interface CreateSignup {
  slotId: number;
  userId: number;
}

export interface UpdatePreferences {
  notifyOnSlotJoin: boolean;
  notifyBeforeSessionStart: boolean;
}
