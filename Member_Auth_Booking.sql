-- SQL Script for Member Authentication & Booking (PostgreSQL version)
-- Database: SpaceHive360_Member

-- Enable UUID extension if not enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Table for storing Member users (Customers)
CREATE TABLE IF NOT EXISTS tbl_member_user (
    rec_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    full_name VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    phone_number VARCHAR(20),
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Table for storing Booking transactions
CREATE TABLE IF NOT EXISTS tbl_member_bookings (
    rec_id UUID PRIMARY KEY DEFAULT uuid_generate_v4(),
    fk_member_user UUID NOT NULL,
    fk_plan UUID NOT NULL,
    plan_type VARCHAR(50) NOT NULL, -- 'Membership' or 'Booking'
    booking_date TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    start_date TIMESTAMP,
    end_date TIMESTAMP,
    total_amount DECIMAL(18, 2),
    payment_status VARCHAR(50) DEFAULT 'Pending', -- Pending, Completed, Failed
    booking_status VARCHAR(50) DEFAULT 'Pending', -- Pending, Confirmed, Cancelled
    payment_id TEXT, -- For tracking payment gateway response
    
    CONSTRAINT FK_MemberUser_Booking FOREIGN KEY (fk_member_user) 
    REFERENCES tbl_member_user(rec_id)
);
