import { TimeAgoPipe } from './time-ago.pipe';

describe('TimeAgoPipe', () => {
  const pipe = new TimeAgoPipe();
  const ago = (seconds: number) => new Date(Date.now() - seconds * 1000).toISOString();

  it('returns empty for missing input', () => {
    expect(pipe.transform(null)).toBe('');
  });

  it('buckets to the largest fitting unit', () => {
    expect(pipe.transform(ago(30))).toMatch(/second/);
    expect(pipe.transform(ago(120))).toMatch(/minute/);
    expect(pipe.transform(ago(2 * 3600))).toMatch(/hour/);
    expect(pipe.transform(ago(3 * 86_400))).toMatch(/day/);
  });
});
