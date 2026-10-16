  (func $j8062 (type 7) (param i32 i32 i32 i32)
    (local i32 i32)
    global.get 0
    i32.const 16
    i32.sub
    local.set 0
    local.get 0
    global.set 0
    local.get 0
    local.get 1
    i32.store offset=12
    i32.const 4507814
    i32.load8_u
    i32.eqz
    if  ;; label = @1
      i32.const 3428556
      call $j870
      i32.const 3430028
      call $j870
      i32.const 3430384
      call $j870
      i32.const 3457636
      call $j870
      i32.const 3457640
      call $j870
      i32.const 3457600
      call $j870
      i32.const 3434312
      call $j870
      i32.const 3434332
      call $j870
      i32.const 3434516
      call $j870
      i32.const 3435048
      call $j870
      i32.const 3435224
      call $j870
      i32.const 3437220
      call $j870
      i32.const 3437592
      call $j870
      i32.const 3438836
      call $j870
      i32.const 3567472
      call $j870
      i32.const 3531032
      call $j870
      i32.const 3538804
      call $j870
      i32.const 3537680
      call $j870
      i32.const 4507814
      i32.const 1
      i32.store8
    end
    i32.const 3434312
    i32.load
    call $j871
    i32.const 3434312
    i32.load
    i32.load offset=92
    i32.load
    local.get 1
    i32.const 0
    call $j38482
    if  ;; label = @1
      i32.const 3438836
      i32.load
      local.set 1
      local.get 0
      i32.const -1
      i32.store offset=4
      local.get 0
      local.get 1
      i32.store
      local.get 0
      local.get 0
      i32.load offset=12
      i32.store offset=8
      local.get 0
      i32.const 0
      call $j956
      local.set 1
      i32.const 3430028
      i32.load
      call $j871
      local.get 1
      i32.const 0
      call $j4354
      local.set 3
      local.get 2
      i32.eqz
      if  ;; label = @2
        i32.const 3434332
        i32.load
        call $j871
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.set 1
        local.get 0
        i32.load offset=12
        local.set 2
        local.get 1
        local.get 2
        local.get 1
        local.get 2
        i32.const 3457636
        i32.load
        call $j1060
        i32.const 1
        i32.sub
        i32.const 3457640
        i32.load
        call $j2275
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.set 2
        local.get 0
        i32.load offset=12
        local.set 5
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=4
        local.get 0
        i32.load offset=12
        i32.const 3457636
        i32.load
        call $j1060
        local.set 1
        local.get 2
        local.get 5
        local.get 1
        i32.const 0
        local.get 1
        i32.const 0
        i32.gt_s
        select
        i32.const 3457640
        i32.load
        call $j2275
      end
      i32.const 3434332
      i32.load
      call $j871
      i32.const 3434332
      i32.load
      i32.load offset=92
      i32.load offset=8
      local.set 1
      local.get 0
      i32.load offset=12
      local.set 2
      local.get 1
      local.get 2
      local.get 1
      local.get 2
      i32.const 3457636
      i32.load
      call $j1060
      i32.const 1
      i32.add
      i32.const 3457640
      i32.load
      call $j2275
      i32.const 3434312
      i32.load
      call $j871
      block  ;; label = @2
        i32.const 3434312
        i32.load
        i32.load offset=92
        i32.load offset=12
        i32.const 4
        i32.eq
        if  ;; label = @3
          i32.const 3437220
          i32.load
          call $j871
          local.get 0
          i32.const 0
          call $j12505
          local.get 0
          i32.const 4
          i32.or
          local.get 0
          i32.const 8
          i32.add
          local.get 0
          i32.load offset=12
          i32.const 3
          i32.sub
          i32.const 2
          i32.lt_u
          select
          i32.load
          local.set 1
          i32.const 3434332
          i32.load
          call $j871
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=8
          local.get 0
          i32.load offset=12
          i32.const 3457636
          i32.load
          call $j1060
          local.get 1
          i32.lt_s
          br_if 1 (;@2;)
          i32.const 3434332
          i32.load
          call $j871
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=12
          local.get 0
          i32.load offset=12
          i32.const 3457600
          i32.load
          call $j20952
          br 1 (;@2;)
        end
        i32.const 3437592
        i32.load
        call $j871
        block  ;; label = @3
          block  ;; label = @4
            local.get 0
            call $j3935
            if  ;; label = @5
              i32.const 3437592
              i32.load
              call $j871
              i32.const 3437592
              i32.load
              i32.load offset=92
              i32.load offset=4
              i32.const 3537680
              i32.load
              i32.const 0
              call $j899
              if  ;; label = @6
                i32.const 3435224
                i32.load
                call $j871
                i32.const 4514392
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3435224
                  call $j870
                  i32.const 4514392
                  i32.const 1
                  i32.store8
                end
                i32.const 3435224
                i32.load
                call $j871
                i32.const 3435224
                i32.load
                i32.load offset=92
                i32.load offset=32
                i32.load offset=188
                local.set 2
                i32.const 3430028
                i32.load
                call $j871
                i32.const 1
                local.set 1
                local.get 2
                i32.const 0
                call $j3091
                local.set 4
                local.get 4
                i32.eqz
                br_if 2 (;@4;)
                local.get 4
                i32.load offset=24
                local.set 2
                local.get 2
                i32.eqz
                br_if 2 (;@4;)
                i32.const 1
                local.set 5
                local.get 2
                i32.load offset=12
                i32.const 2
                i32.lt_s
                br_if 3 (;@3;)
                local.get 4
                i32.load offset=24
                i32.load offset=20
                local.set 1
                local.get 2
                i32.load offset=16
                local.set 5
                br 3 (;@3;)
              end
              i32.const 3437592
              i32.load
              call $j871
              i32.const 3437592
              i32.load
              i32.load offset=92
              i32.load offset=4
              i32.const 3531032
              i32.load
              i32.const 0
              call $j899
              if  ;; label = @6
                i32.const 3430384
                i32.load
                call $j871
                i32.const 1
                local.set 1
                i32.const 0
                call $j6856
                local.set 4
                local.get 4
                i32.eqz
                br_if 2 (;@4;)
                local.get 4
                i32.load offset=24
                local.set 2
                local.get 2
                i32.eqz
                br_if 2 (;@4;)
                i32.const 1
                local.set 5
                local.get 2
                i32.load offset=12
                i32.const 2
                i32.lt_s
                br_if 3 (;@3;)
                local.get 4
                i32.load offset=24
                i32.load offset=20
                local.set 1
                local.get 2
                i32.load offset=16
                local.set 5
                br 3 (;@3;)
              end
              i32.const 3437592
              i32.load
              call $j871
              i32.const 3437592
              i32.load
              i32.load offset=92
              i32.load offset=4
              i32.const 3538804
              i32.load
              i32.const 0
              call $j899
              if  ;; label = @6
                i32.const 3435048
                i32.load
                call $j871
                i32.const 4509456
                i32.load8_u
                i32.eqz
                if  ;; label = @7
                  i32.const 3435048
                  call $j870
                  i32.const 4509456
                  i32.const 1
                  i32.store8
                end
                i32.const 3435048
                i32.load
                call $j871
                i32.const 1
                local.set 1
                i32.const 3435048
                i32.load
                i32.load offset=92
                i32.load offset=12
                local.set 4
                local.get 4
                i32.eqz
                br_if 2 (;@4;)
                local.get 4
                i32.load offset=24
                local.set 2
                local.get 2
                i32.eqz
                br_if 2 (;@4;)
                i32.const 1
                local.set 5
                local.get 2
                i32.load offset=12
                i32.const 2
                i32.lt_s
                br_if 3 (;@3;)
                local.get 4
                i32.load offset=24
                i32.load offset=20
                local.set 1
                local.get 2
                i32.load offset=16
                local.set 5
                br 3 (;@3;)
              end
              i32.const 3437592
              i32.load
              call $j871
              i32.const 3437592
              i32.load
              i32.load offset=92
              i32.load offset=4
              local.set 1
              i32.const 3434516
              i32.load
              call $j871
              local.get 1
              i32.const 0
              call $j8043
              if  ;; label = @6
                i32.const 3434516
                i32.load
                call $j871
                i32.const 1
                local.set 1
                i32.const 3434516
                i32.load
                i32.load offset=92
                i32.load offset=20
                local.set 4
                local.get 4
                i32.eqz
                br_if 2 (;@4;)
                local.get 4
                i32.load offset=36
                local.set 2
                local.get 2
                i32.eqz
                br_if 2 (;@4;)
                i32.const 1
                local.set 5
                local.get 2
                i32.load offset=12
                i32.const 2
                i32.lt_s
                br_if 3 (;@3;)
                local.get 4
                i32.load offset=36
                i32.load offset=20
                local.set 1
                local.get 2
                i32.load offset=16
                local.set 5
                br 3 (;@3;)
              end
              i32.const 3437592
              i32.load
              call $j871
              i32.const 3437592
              i32.load
              i32.load offset=92
              i32.load offset=4
              local.set 1
              i32.const 3437220
              i32.load
              call $j871
              local.get 0
              local.get 1
              i32.const 0
              call $j18184
              local.get 0
              i32.load
              i32.const 1
              i32.and
              i32.eqz
              if  ;; label = @6
                i32.const 5
                i32.const 3567472
                i32.load
                i32.const 0
                call $j1464
                i32.const 3437220
                i32.load
                call $j871
                local.get 0
                i32.const 0
                call $j12506
              end
              local.get 0
              i32.load offset=4
              local.set 1
              local.get 0
              i32.load offset=8
              local.set 5
              br 2 (;@3;)
            end
            i32.const 3434332
            i32.load
            call $j871
            i32.const 3434332
            i32.load
            i32.load offset=92
            i32.load offset=8
            local.get 0
            i32.load offset=12
            i32.const 3457636
            i32.load
            call $j1060
            local.get 3
            i32.load offset=24
            i32.lt_s
            br_if 2 (;@2;)
            i32.const 3434332
            i32.load
            call $j871
            i32.const 3434332
            i32.load
            i32.load offset=92
            i32.load offset=12
            local.get 0
            i32.load offset=12
            i32.const 3457600
            i32.load
            call $j20952
            br 2 (;@2;)
          end
          i32.const 1
          local.set 5
        end
        local.get 5
        local.set 2
        local.get 0
        i32.load offset=12
        local.set 4
        i32.const 3434332
        i32.load
        call $j871
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=8
        local.get 0
        i32.load offset=12
        i32.const 3457636
        i32.load
        call $j1060
        local.get 1
        local.get 2
        local.get 4
        i32.const 3
        i32.sub
        i32.const 2
        i32.lt_u
        select
        i32.lt_s
        br_if 0 (;@2;)
        i32.const 3434332
        i32.load
        call $j871
        i32.const 3434332
        i32.load
        i32.load offset=92
        i32.load offset=12
        local.get 0
        i32.load offset=12
        i32.const 3457600
        i32.load
        call $j20952
      end
      i32.const 3428556
      i32.load
      call $j871
      i32.const 3428556
      i32.load
      i32.load offset=92
      i32.load offset=60
      local.set 1
      local.get 1
      if  ;; label = @2
        local.get 1
        local.get 0
        i32.load offset=12
        i32.const -1
        call $j3411
      end
      i32.const 3438836
      i32.load
      local.set 1
      local.get 0
      i32.const -1
      i32.store offset=4
      local.get 0
      local.get 1
      i32.store
      local.get 0
      local.get 0
      i32.load offset=12
      i32.store offset=8
      local.get 0
      i32.const 0
      call $j956
      local.set 1
      i32.const 3435224
      i32.load
      call $j871
      local.get 1
      i32.const 0
      call $j3052
      i32.const 1
      i32.ge_s
      if  ;; label = @2
        i32.const 3438836
        i32.load
        local.set 1
        local.get 0
        i32.const -1
        i32.store offset=4
        local.get 0
        local.get 1
        i32.store
        local.get 0
        local.get 0
        i32.load offset=12
        i32.store offset=8
        local.get 0
        i32.const 0
        call $j956
        local.set 1
        i32.const 3435224
        i32.load
        call $j871
        local.get 1
        i32.const -1
        i32.const 0
        i32.const 0
        i32.const 0
        call $j3712
      end
      block  ;; label = @2
        local.get 3
        i32.load offset=44
        i32.const 1
        i32.ge_s
        if  ;; label = @3
          i32.const 3434332
          i32.load
          call $j871
          i32.const 3434332
          i32.load
          i32.load offset=92
          i32.load offset=8
          local.get 0
          i32.load offset=12
          i32.const 3457636
          i32.load
          call $j1060
          local.get 3
          i32.load offset=44
          call $i32s_rem
          br_if 1 (;@2;)
        end
        local.get 0
        local.get 0
        local.get 0
        call $j19855
      end
      local.get 0
      local.get 0
      call $j3725
    end
    local.get 0
    i32.const 16
    i32.add
    global.set 0)
